require("@testing-library/jest-dom");

const render = require("@testing-library/svelte").render;
const fireEvent = require("@testing-library/svelte").fireEvent;

const Menu = require("../svelte-app/Menu");

test("Checks if the login button is visible while logged out", () => {
    const { queryByText } = render(Menu);

    expect(queryByText("Inloggen")).toBeInTheDocument();
});

test("Checks if the logout button is not visible while logged out", () => {
    const { queryByTitle } = render(Menu);

    expect(queryByTitle("uitloggen")).not.toBeInTheDocument();
});