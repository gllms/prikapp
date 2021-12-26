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
        fetch(event.request).then(async response => {
            const cache = await caches.open(CACHE_NAME);
            cache.put(event.request.url, response.clone());
            return response;
        }).catch(async () => {
            const response = await caches.match(event.request);
            if (response)
                return response;
            if (new URL(event.request.url).pathname.split("/").pop().indexOf(".") < 0)
                return caches.match("/");
        })
    );
});