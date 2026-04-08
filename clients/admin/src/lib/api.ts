import { createApiClient, createTenantEndpoints, createUserEndpoints, createRoleEndpoints, createAuthEndpoints } from "@fsh/api-client";
import { getSession } from "next-auth/react";

const apiClient = createApiClient({
  baseUrl: process.env.NEXT_PUBLIC_FSH_API_URL ?? "http://localhost:5000",
  getAccessToken: async () => {
    const session = await getSession();
    return session?.accessToken ?? null;
  },
  getTenantId: () => {
    if (typeof window === "undefined") return null;
    return localStorage.getItem("fsh-active-tenant") ?? null;
  },
});

export const api = {
  auth: createAuthEndpoints(apiClient),
  tenants: createTenantEndpoints(apiClient),
  users: createUserEndpoints(apiClient),
  roles: createRoleEndpoints(apiClient),
};

export { apiClient };
