export default function injectScript(file_path) {
    const script = document.createElement('script');
    script.setAttribute('type', 'text/javascript');
    script.setAttribute('src', file_path);
    document.head.appendChild(script);
}