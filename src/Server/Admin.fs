namespace APFS.Server

open System.Text.Json
open System.Text.Json.Serialization

module Admin =

    type PublicKey = {
        [<JsonPropertyName("id")>] Id: string
        [<JsonPropertyName("owner")>] Owner: string
        [<JsonPropertyName("publicKeyPem")>] PublicKeyPem: string
    }

    type Actor = {
        [<JsonPropertyName("@context")>] Context: string array
        [<JsonPropertyName("id")>] Id: string
        [<JsonPropertyName("type")>] ActorType: string
        [<JsonPropertyName("preferredUsername")>] PreferredUsername: string
        [<JsonPropertyName("inbox")>] Inbox: string
        [<JsonPropertyName("followers")>] Followers: string
        [<JsonPropertyName("publicKey")>] PublicKey: PublicKey
    }

    let createActor name domain pubKey = 
        {
            Context= [|
                "https://www.w3.org/ns/activitystreams"
                "https://w3id.org/security/v1" 
            |]
            Id = (sprintf "https://%s/u/%s" domain name)
            ActorType = "Person"
            PreferredUsername = name
            Inbox = (sprintf "https://%s/api/inbox" domain)
            Followers = (sprintf "https://%s/u/%s/followers" domain name)
            PublicKey = {
                Id=(sprintf "https://%s/u/%s#main-key" domain name)
                Owner=(sprintf "https://%s/u/%s" domain name)
                PublicKeyPem = pubKey
            }
        }
    
module AdminRoutes =

    open Admin
    open Saturn
    open Giraffe
    open FSharp.Control.Tasks.ContextInsensitive

    type Account = {
        Name: string
    }

    let postCreateActor : HttpHandler = 
        handleContext(
            fun ctx -> 
                task {
                    let! account = ctx.BindJsonAsync<Account>()
                    let domain = ctx.Request.Host.ToString()
                    let acct = createActor account.Name domain (System.Guid.NewGuid().ToString())
                    return! ctx.WriteJsonAsync acct
                }
        )

    let Router = router {
        post "/create" postCreateActor
    }

