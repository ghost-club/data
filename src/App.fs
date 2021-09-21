module App

open System
open Fable.Core
open Fable.Core.JsInterop
open Node.Api

let [<Literal>] BaseUrl = "https://xn--pckjp4dudxftf.xn--tckwe/data/"
let [<Literal>] GoogleAppUrl = "https://script.google.com/macros/s/AKfycbxexm2XgQur4GfAnH9FRcqdwLGlmZdnHBqbIfsZhLuiToCL0mwH0SH0p1DB4wVG64iT/exec"

type [<AllowNullLiteral>] IResult<'T> =
  abstract status: string with get, set

[<RequireQualifiedAccess>]
module IResult =
  type [<AllowNullLiteral>] Ok<'T> =
    inherit IResult<'T>
    abstract value: 'T with get, set

  type [<AllowNullLiteral>] Error<'T> =
    inherit IResult<'T>
    abstract message: string with get, set

  let inline Ok (x: 'a) : IResult<'a> = !!{| status = "ok"; value = x |}
  let inline Error (msg: string)  : IResult<'a> = !!{| status = "error"; message = msg |}

  let inline isOk (x: IResult<_>) = x.status = "ok"
  let inline isError (x: IResult<_>) = x.status <> "ok" && x.status <> "loading"

  let inline (|Ok|Error|) (x: IResult<'T>) =
    match x.status with
    | "ok" ->
      Ok (x :?> Ok<'T>).value
    | _ ->
      Error (x :?> Error<'T>).message

type IMediaInfo = {|
  id: string
  baseUrl: string
  mimeType: string
  created: DateTimeOffset
  width: int
  height: int
  thumbnailUrl: string
  origUrl: string
  srcSet: string
  thumbnailUrlWebP: string
  origUrlWebP: string
  srcSetWebP: string
|}

module IMediaInfo =
  let inline getUrlWithSize (width: int) (height: int) (x: IMediaInfo) =
    sprintf "%s=w%d-h%d" x.baseUrl width height
  let inline getThumbUrl (x: IMediaInfo) =
    if isNullOrUndefined x.thumbnailUrl then x.baseUrl
    else x.thumbnailUrl
  let inline getOrigUrl (x: IMediaInfo) =
    if isNullOrUndefined x.origUrl then
      sprintf "%s=w%d-h%d" x.baseUrl x.width x.height
    else
      x.origUrl

type [<AllowNullLiteral>] IPoem =
  [<Emit("$0[0]")>]
  abstract Japanese: string
  [<Emit("$0[1]")>]
  abstract English: string

module IPoem =
  let inline create (ja: string) (en: string) : IPoem = !!(ja, en)

type All = {| images: IMediaInfo[]; poems: IPoem[] |}

let inline private parseJson txt =
  JS.JSON.parse(txt, (fun key value ->
    if (key :?> string) = "created" then
      JS.Constructors.Date.Create(value :?> string) |> box
    else value
  )) :?> 'a

let sizes = [ "small", 480; "medium", 960; "large", 1920; ]

let getValidSizes (pic: IMediaInfo) =
  sizes
  |> List.filter (fun (_, width) -> width < pic.width)

let createFileNameOfMedia (pic: IMediaInfo) suffix ext = sprintf "%s-%s.%s" pic.id suffix ext

open Sharp

let processImageTask (pic: IMediaInfo) =
  let write (orig: Node.Buffer.Buffer) suffix (config: Sharp.Sharp -> Sharp.Sharp) =
    promise {
      let s1 = Sharp.Create(orig) |> config
      do! s1.jpeg(!!{| progressive = true |}).toFile("./result/gallery/" + createFileNameOfMedia pic suffix "jpg") |> Promise.map ignore
      let s2 = Sharp.Create(orig) |> config
      do! s2.webp(!!{| quality = 85; reductionEffort = 6 |}).toFile("./result/gallery/" + createFileNameOfMedia pic suffix "webp") |> Promise.map ignore
    }

  promise {
    let! orig =
      Fetch.fetch (IMediaInfo.getOrigUrl pic) []
    let! orig = orig.buffer()

    do! write orig "orig" id
    do! write orig "thumb" (fun s -> s.resize(240.0).blur(!^10.0))

    for name, width in getValidSizes pic do
      do! write orig name (fun s -> s.resize(float width))
  }

let mapMediaInfo (pic: IMediaInfo) : IMediaInfo =
  let getUrl suffix ext = BaseUrl + "gallery/" + createFileNameOfMedia pic suffix ext
  let createSrcSet ext =
    let entries =
      getValidSizes pic
      |> List.map (fun (suffix, width) ->
        sprintf "%s %dw" (getUrl suffix ext) width)
    let orig =
      sprintf "%s %dw" (getUrl "orig" ext) pic.width
    (entries @ [orig]) |> String.concat ","
  {| pic with
      srcSet = createSrcSet "jpg"
      srcSetWebP = createSrcSet "webp"
      origUrl = getUrl "orig" "jpg"
      origUrlWebP = getUrl "orig" "webp"
      thumbnailUrl = getUrl "thumb" "jpg"
      thumbnailUrlWebP = getUrl "thumb" "webp"
  |}

let checkUpdated (input: IResult<All>) (oldOutput: IResult<All>) =
  match input, oldOutput with
  | IResult.Error _, _ | _, IResult.Error _ -> true
  | IResult.Ok input, IResult.Ok output ->
    if input.poems <> output.poems then true
    else
      let s1 = input.images |> Seq.map (fun m -> m.id) |> Set.ofSeq
      let s2 = output.images |> Seq.map (fun m -> m.id) |> Set.ofSeq
      if s1 <> s2 then true
      else
        output.images |> Array.exists (fun m -> isNullOrUndefined m.srcSet || isNullOrUndefined m.thumbnailUrlWebP)

open Fetch.Types

let rec mainTask (retryCount: int) =
  promise {
    if fs.existsSync(!^"./result/") |> not then fs.mkdirSync("./result/")
    printfn "fetching data..."
    let! inputResult =
      Fetch.tryFetch (GoogleAppUrl + "?action=all-force") [
        Redirect RedirectMode.Follow
        Fetch.requestHeaders [
          UserAgent "GC Website Updater"
        ]
      ]
    let! oldOutput =
      Fetch.fetch (BaseUrl + "index.json") []
    match inputResult with
    | Error e ->
      printfn "failed to fetch data (retry: %d): %s" retryCount e.Message
      if retryCount < 3 then
        do! Promise.sleep 1000
        return! mainTask (retryCount + 1)
      else
        printfn "%s" (e.ToString())
        return -1
    | Ok input ->
      if not input.Ok then
        printfn "failed to fetch data"
        return -1
      else
        let! input = input.json<IResult<All>>()
        let! shouldUpdate =
          promise {
            if not oldOutput.Ok then return true
            else
              let! oldOutput = oldOutput.json<IResult<All>>()
              return checkUpdated input oldOutput
          }
        if not shouldUpdate then
          printfn "does not need to be updated"
          ``process``.env?NO_UPDATE <- "true"
          return 0
        else
          match input with
          | IResult.Error msg ->
            printfn "%s" msg
            return -1
          | IResult.Ok all ->
            printfn "generating gallery..."
            if fs.existsSync(!^"./result/gallery") |> not then fs.mkdirSync("./result/gallery/")
            let! _ =
              Promise.all [
                for pic in all.images do
                  yield processImageTask pic
              ]

            printfn "creating index.json..."
            let all =
              let images =
                all.images
                |> Array.map mapMediaInfo
              {| all with images = images |}
            let newJson =
              JS.JSON.stringify(IResult.Ok all)
            fs.writeFileSync("./result/index.json", newJson)
            return 0
  }

mainTask 0
|> Promise.catch (fun e ->
  eprintf "%s" (e.ToString()); -1)
|> Promise.iter (fun i -> if i <> 0 then ``process``.exit i)
