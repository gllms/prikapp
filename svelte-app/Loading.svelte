<div class="loading"><div></div><div></div><div></div><div></div></div>

<style>
    .loading {
        width: 80px;
        height: 80px;
        margin: 0 auto;
    }

    .loading div {
        box-sizing: border-box;
        display: block;
        position: absolute;
        width: 64px;
        height: 64px;
        margin: 8px;
        border: 8px solid #e7334c;
        border-radius: 50%;
        animation: loading 1.2s cubic-bezier(0.57, 0.13, 0.38, 0.8) infinite;
        border-color: #e7334c transparent transparent transparent;
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