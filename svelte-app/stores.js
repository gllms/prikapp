import { writable } from "svelte/store";

export const cards = writable([]);

export const textSize = writable(localStorage.getItem("textSize") || "100%");
textSize.subscribe(value => localStorage.setItem("textSize", value));
export const themeChoice = writable(localStorage.getItem("theme") || "light");
themeChoice.subscribe(value => localStorage.setItem("theme", value));

export const overlayCount = writable(0);
export const currentPage = writable("");

export const token = writable(localStorage.getItem("token"));
token.subscribe(value => localStorage.setItem("token", value));