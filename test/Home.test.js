require("@testing-library/jest-dom");
global.fetch = require("node-fetch");

const render = require("@testing-library/svelte").render
const fireEvent = require("@testing-library/svelte").fireEvent
const Comp = require("../svelte-app/Home");
 
 
test("Negative test to check if there are more than -1 cards", () => {
    const {container} = render(Comp)
    const cardCount = container.getElementsByTagName("div")[0].childNodes.length
   
    expect(cardCount).not.toBeLessThan(0)
})