import { writable } from "svelte/store";

export const ageChoice = writable(localStorage.getItem("age"));
export const freqChoice = writable(localStorage.getItem("freq"));
export const themeChoice = writable(localStorage.getItem("theme"));

export const overlayCount = writable(0);
export const currentPage = writable("");