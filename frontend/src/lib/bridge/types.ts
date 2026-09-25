/**
 * Bridge contract mirror. These types must stay aligned with the C# side
 * (MasoloAgro.App/WebView/WebViewMessages.cs). The frontend never invents
 * operation names and never embeds business rules here.
 */

export const BridgeOperations = {
  authLogin: "auth.login",
  authLogout: "auth.logout",
  authCurrentSession: "auth.currentSession",

  commoditiesList: "commodities.list",
  commoditiesGet: "commodities.get",
  commoditiesCreate: "commodities.create",
  commoditiesUpdate: "commodities.update",

  salesList: "sales.list",
  salesGet: "sales.get",
  salesCreate: "sales.create",
  salesReverse: "sales.reverse",

  purchasesList: "purchases.list",
  purchasesGet: "purchases.get",
  purchasesCreate: "purchases.create",
  purchasesReverse: "purchases.reverse",

  stockSummary: "stock.summary",
  stockLedger: "stock.ledger",
  stockAdjust: "stock.adjust",

  reportsSales: "reports.sales",
  reportsPurchases: "reports.purchases",
  reportsStock: "reports.stock",

  usersList: "users.list",
  usersCreate: "users.create",
  usersUpdate: "users.update",
  usersDisable: "users.disable",
} as const;

export type BridgeOperation =
  (typeof BridgeOperations)[keyof typeof BridgeOperations];

export interface BridgeRequest {
  operation: BridgeOperation;
  payload: string | null;
}

export interface BridgeResponse {
  ok: boolean;
  payload: string | null;
  error: string | null;
}

export class BridgeError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "BridgeError";
  }
}
