const OFFLINE_VERSION = 1;
const CACHE_NAME = "offline";

self.addEventListener("install", (event) => {
    event.waitUntil(
        (async () => {
            const cache = await caches.open(CACHE_NAME);
            return await cache.addAll(
                [
                    "/",
                    "/images/icons-192.png",
                    "https://fonts.googleapis.com/icon?family=Material+Icons",
                    "/global.css",
                    "/build/bundle.css",
                    "/build/bundle.js",
                    "/cards.json",
                    "/locations.json",
                    "/postcodes.json"
                ].map((url) => new Request(url, { cache: "reload" }))
            );
        })()
    );
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        (async () => {
            if ("navigationPreload" in self.registration) {
                await self.registration.navigationPreload.enable();
            }
        })()
    );

    self.clients.claim();
});

self.addEventListener("fetch", event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }
                return fetch(event.request).then(response => {
                    return caches.open(CACHE_NAME).then(cache => {
                        cache.put(event.request.url, response.clone());
                        return response;
                    });
                });
            }).catch(error => {
                if (new URL(event.request.url).pathname.split("/").pop().indexOf(".") < 0) {
                    return caches.match("/");
                }
            })
    );
});