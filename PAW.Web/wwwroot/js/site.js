// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function getExchangeRate() {
//    const myHeaders = new Headers();
//    myHeaders.append("UserAgent", "curl/8.7.1");
//    myHeaders.append("Accept", "*/*");

//    const requestOptions = {
//        method: "GET",
//        headers: myHeaders,
//        redirect: "follow"
//    };

//    fetch("https://api.hacienda.go.cr/indicadores/tc", requestOptions)
//        .then((response) => response.text())
//        .then((result) => {
//            return result.json();
//        })
//        .catch((error) => console.error(error));
//}

// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// =====================================================================
// Tipo de cambio (API de Hacienda) cargado desde el navegador, con caché
// =====================================================================

const EXCHANGE_RATE_URL = "https://api.hacienda.go.cr/indicadores/tc";
const EXCHANGE_RATE_CACHE_KEY = "paw.exchangeRate";
const EXCHANGE_RATE_TTL_MS = 60 * 60 * 1000; // 1 hora
const EXCHANGE_RATE_COLLAPSED_KEY = "paw.exchangeRate.collapsed";

// Caché en memoria de la petición en curso: si se llama getExchangeRate()
// varias veces mientras la primera no ha respondido, todas comparten la misma.
let exchangeRateRequest = null;

// Lee el caché de localStorage. Con ignoreTtl = true devuelve aunque esté vencido
// (se usa como respaldo si la API no responde).
function readExchangeRateCache(ignoreTtl = false) {
    try {
        const raw = localStorage.getItem(EXCHANGE_RATE_CACHE_KEY);
        if (!raw) return null;

        const cache = JSON.parse(raw);
        const isExpired = Date.now() - cache.savedAt > EXCHANGE_RATE_TTL_MS;
        if (isExpired && !ignoreTtl) return null;

        return cache;
    } catch {
        return null; // localStorage bloqueado o JSON dañado
    }
}

function writeExchangeRateCache(data) {
    const cache = { savedAt: Date.now(), data: data };
    try {
        localStorage.setItem(EXCHANGE_RATE_CACHE_KEY, JSON.stringify(cache));
    } catch {
        // Sin localStorage (modo privado, etc.): seguimos sin caché.
    }
    return cache;
}

// Devuelve { savedAt, data, source } donde source es "cache", "api" o "stale".
async function getExchangeRate(forceRefresh = false) {
    if (!forceRefresh) {
        const cached = readExchangeRateCache();
        if (cached) return { ...cached, source: "cache" };
    }

    if (!exchangeRateRequest) {
        // Sin headers personalizados: así el navegador no hace preflight CORS.
        // (User-Agent no se puede cambiar desde JS, el navegador lo bloquea.)
        exchangeRateRequest = fetch(EXCHANGE_RATE_URL, { headers: { Accept: "application/json" } })
            .then((response) => {
                if (!response.ok) throw new Error(`HTTP ${response.status}`);
                return response.json();
            })
            .then((data) => ({ ...writeExchangeRateCache(data), source: "api" }))
            .catch((error) => {
                console.error("Error al obtener el tipo de cambio:", error);
                const stale = readExchangeRateCache(true);
                if (stale) return { ...stale, source: "stale" };
                throw error;
            })
            .finally(() => {
                exchangeRateRequest = null;
            });
    }

    return exchangeRateRequest;
}

// ---------- Formato ----------

const crcFormat = new Intl.NumberFormat("es-CR", { style: "currency", currency: "CRC", minimumFractionDigits: 2 });
const usdFormat = new Intl.NumberFormat("es-CR", { style: "currency", currency: "USD", minimumFractionDigits: 4 });

// La API devuelve "2026-10-04". new Date("2026-10-04") lo toma como UTC y en
// Costa Rica mostraría el día anterior, así que se arma como fecha local.
function formatApiDate(value) {
    if (!value) return "";
    const [year, month, day] = value.substring(0, 10).split("-").map(Number);
    return new Date(year, month - 1, day).toLocaleDateString("es-CR", { day: "numeric", month: "short", year: "numeric" });
}

function formatTime(timestamp) {
    return new Date(timestamp).toLocaleTimeString("es-CR", { hour: "2-digit", minute: "2-digit" });
}

// ---------- Pintar el widget ----------

function setFx(widget, key, text) {
    const el = widget.querySelector(`[data-fx="${key}"]`);
    if (el) el.textContent = text;
}

function renderExchangeRate(widget, result) {
    const { data, savedAt, source } = result;

    setFx(widget, "dolarCompra", crcFormat.format(data.dolar.compra.valor));
    setFx(widget, "dolarVenta", crcFormat.format(data.dolar.venta.valor));
    setFx(widget, "euroColones", crcFormat.format(data.euro.colones));
    setFx(widget, "euroDolares", usdFormat.format(data.euro.dolares));
    setFx(widget, "fecha", `Hacienda · ${formatApiDate(data.dolar.venta.fecha)}`);

    const sourceText = {
        api: `En vivo · ${formatTime(savedAt)}`,
        cache: `Caché · ${formatTime(savedAt)}`,
        stale: `Sin conexión · ${formatTime(savedAt)}`
    };
    setFx(widget, "origen", sourceText[source]);

    widget.classList.remove("is-loading", "is-error");
    widget.classList.toggle("is-stale", source === "stale");
}

function renderExchangeRateError(widget) {
    widget.classList.remove("is-loading");
    widget.classList.add("is-error");
    setFx(widget, "fecha", "No se pudo cargar");
    setFx(widget, "origen", "Usá ↻ para reintentar");
}

// Método principal: carga el tipo de cambio y lo muestra en el widget.
async function loadExchangeRate(forceRefresh = false) {
    const widget = document.getElementById("exchange-rate");
    if (!widget) return;

    widget.classList.add("is-loading");
    try {
        const result = await getExchangeRate(forceRefresh);
        renderExchangeRate(widget, result);
    } catch {
        renderExchangeRateError(widget);
    }
}

// ---------- Botones (actualizar / minimizar) ----------

function setExchangeRateCollapsed(widget, collapsed) {
    widget.classList.toggle("is-collapsed", collapsed);
    const toggle = widget.querySelector('[data-fx-action="toggle"]');
    if (toggle) {
        toggle.setAttribute("aria-expanded", String(!collapsed));
        toggle.setAttribute("title", collapsed ? "Expandir" : "Minimizar");
        toggle.innerHTML = collapsed ? "&plus;" : "&minus;";
    }
    try {
        localStorage.setItem(EXCHANGE_RATE_COLLAPSED_KEY, collapsed ? "1" : "0");
    } catch { }
}

function initExchangeRateWidget() {
    const widget = document.getElementById("exchange-rate");
    if (!widget) return;

    let collapsed = false;
    try {
        collapsed = localStorage.getItem(EXCHANGE_RATE_COLLAPSED_KEY) === "1";
    } catch { }
    setExchangeRateCollapsed(widget, collapsed);

    widget.addEventListener("click", (event) => {
        const button = event.target.closest("[data-fx-action]");
        if (!button) return;

        if (button.dataset.fxAction === "refresh") {
            loadExchangeRate(true); // ignora el caché
        } else if (button.dataset.fxAction === "toggle") {
            setExchangeRateCollapsed(widget, !widget.classList.contains("is-collapsed"));
        }
    });

    loadExchangeRate();
}

document.addEventListener("DOMContentLoaded", initExchangeRateWidget);
