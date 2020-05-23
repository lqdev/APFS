open Saturn
open Giraffe
open APFS.Server

let appRouter = router {
    forward "/admin" AdminRoutes.Router
    get "/" (text "Hello World!")
}

let app = application {
    use_router appRouter
}

run app