import {
  BridgeError,
  type BridgeOperation,
  type BridgeResponse,
} from "./types.ts";

interface WebViewHost {
  postMessage(message: string): void;
  addEventListener(
    type: "message",
    listener: (event: { data: string }) => void,
  ): void;
}

declare global {
  interface Window {
    chrome?: {
      webview?: WebViewHost;
    };
  }
}

let nextRequestId = 1;
const pending = new Map<number, (response: BridgeResponse) => void>();
let listening = false;

function getHost(): WebViewHost {
  const host = window.chrome?.webview;
  if (!host) {
    throw new BridgeError(
      "The desktop bridge is unavailable. Run inside the Masolo Agro app to use business data.",
    );
  }
  return host;
}

function ensureListening(host: WebViewHost): void {
  if (listening) {
    return;
  }
  listening = true;
  host.addEventListener("message", (event) => {
    let envelope: { id?: number; response?: BridgeResponse };
    try {
      envelope = JSON.parse(event.data) as typeof envelope;
    } catch {
      return;
    }
    if (typeof envelope.id !== "number") {
      return;
    }
    const resolve = pending.get(envelope.id);
    if (resolve && envelope.response) {
      pending.delete(envelope.id);
      resolve(envelope.response);
    }
  });
}

/**
 * Calls one explicit C# bridge operation and returns its raw JSON payload.
 * Callers parse the payload into their own response type. Rejects with a
 * BridgeError when the bridge is missing or the operation fails.
 */
export function invokeBridge(
  operation: BridgeOperation,
  payload: unknown = null,
): Promise<string | null> {
  const host = getHost();
  ensureListening(host);

  const id = nextRequestId++;
  const request = {
    id,
    request: {
      operation,
      payload: payload === null ? null : JSON.stringify(payload),
    },
  };

  return new Promise<string | null>((resolve, reject) => {
    pending.set(id, (response) => {
      if (response.ok) {
        resolve(response.payload);
      } else {
        reject(new BridgeError(response.error ?? "The request failed."));
      }
    });
    host.postMessage(JSON.stringify(request));
  });
}
