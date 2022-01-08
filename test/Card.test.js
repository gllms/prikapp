/**
 * @jest-environment jsdom
 */

// NOTE: jest-dom adds handy assertions to Jest and it is recommended, but not required.
//import '@testing-library/jest-dom'
require('@testing-library/jest-dom');


//import {render, fireEvent} from '@testing-library/svelte'

const render = require('@testing-library/svelte').render
const fireEvent = require('@testing-library/svelte').fireEvent

//import Comp from '../svelte-app/Card'
const Comp = require('../svelte-app/Card');

const data = {
    Type: 0,
    Title: "De weg van het bloed",
    Description: "In deze video wordt uitgelegd wat er allemaal met jouw bloed gebeurt",
    Content: "In deze video wordt uitgelegd wat er allemaal met jouw bloed gebeurt"
}

test('Checks if the cards contains the right information after creation', () => {
    const {getByText} = render(Comp, {card: data})

    expect(getByText(data["Description"])).toBeInTheDocument()
})