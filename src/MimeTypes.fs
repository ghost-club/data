// ts2fable 0.7.1
module rec MimeTypes
open System
open Fable.Core
open Fable.Core.JS

let [<Import("types","mime-types")>] types: Types = jsNative
let [<Import("extensions","mime-types")>] extensions: Extensions = jsNative
let [<ImportDefault("mime-types")>] mime : IExports = jsNative

type [<AllowNullLiteral>] IExports =
    abstract lookup: filenameOrExt: string -> string
    abstract contentType: filenameOrExt: string -> string
    abstract extension: typeString: string -> string
    abstract charset: typeString: string -> string

type [<AllowNullLiteral>] Types =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> string with get, set

type [<AllowNullLiteral>] Extensions =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> ResizeArray<string> with get, set
