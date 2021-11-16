import App from './App.svelte';

const app = new App({
	target: document.body,
    props: {
        getAllLocations: async function() {
            var locations = [];
            let page = await  fetch("https://api.codetabs.com/v1/proxy/?quest=https://www.star-shl.nl/locaties-functiebeeldvormend-onderzoek/")
            .then(function (response) {
                // The API call was successful!
                return response.text();
            }).then(function (html) {
                // This is the HTML from our response as a text string
                //console.log(html);
            }).catch(function (err) {
                // There was an error
                console.warn('Something went wrong.', err);
            });

        
            return locations
        },
    }
});



export default app;