[<AutoOpen>]
module Utils

let inline (|Ref|) (x: 'a ref) = !x

let inline (|NullOrUndefined|_|) x =
  if Fable.Core.JsInterop.isNullOrUndefined x then Some ()
  else None
