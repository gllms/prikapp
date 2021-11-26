import { writable } from "svelte/store";

if (localStorage.getItem("age")==null) {
    localStorage.setItem("age", "");
}

if (localStorage.getItem("freq")==null) {
    localStorage.setItem("freq", "");
}

if (localStorage.getItem("theme")==null) {
    localStorage.setItem("theme", "");
}

export const ageChoice = writable(localStorage.getItem("age"));
export const freqChoice = writable(localStorage.getItem("freq"));
export const themeChoice = writable(localStorage.getItem("theme"));
         
