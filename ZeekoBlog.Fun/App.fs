module App
open Giraffe

let webApp =
    choose [
        route "/" >=> text "index"
    ]