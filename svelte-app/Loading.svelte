<script>
    export let white = false;
</script>

<div class="loading" class:white={white}><div></div><div></div><div></div><div></div></div>

<style>
    .loading {
        width: 40px;
        height: 40px;
        margin: 0 auto;
        grid-column: span 2;
    }

    .loading div {
        box-sizing: border-box;
        display: block;
        position: absolute;
        width: 32px;
        height: 32px;
        margin: 4px;
        border: 4px solid #e7334c;
        border-radius: 50%;
        animation: loading 1.2s cubic-bezier(0.57, 0.13, 0.38, 0.8) infinite;
        border-color: #e7334c transparent transparent transparent;
    }

    .loading.white div {
        border-color: white transparent transparent transparent;
    }

    .loading div:nth-child(1) {
        animation-delay: -0.45s;
    }

    .loading div:nth-child(2) {
        animation-delay: -0.3s;
    }

    .loading div:nth-child(3) {
        animation-delay: -0.15s;
    }

    @keyframes loading {
        0% {
            transform: rotate(0deg);
        }
        100% {
            transform: rotate(360deg);
        }
    }
</style>