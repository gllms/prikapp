import { writable } from "svelte/store";

export const ageChoice = writable(localStorage.getItem("age"));
ageChoice.subscribe(value => localStorage.setItem("age", value));
export const freqChoice = writable(localStorage.getItem("freq"));
freqChoice.subscribe(value => localStorage.setItem("freq", value));
export const themeChoice = writable(localStorage.getItem("theme"));
themeChoice.subscribe(value => localStorage.setItem("theme", value));

export const overlayCount = writable(0);
export const currentPage = writable("");