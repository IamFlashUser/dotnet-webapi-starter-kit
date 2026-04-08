import type { DefaultSession } from "next-auth";

export interface FshUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  tenantId: string;
  imageUrl?: string;
  permissions: string[];
  isSuperAdmin: boolean;
}

declare module "next-auth" {
  interface Session extends DefaultSession {
    user: FshUser;
    accessToken: string;
    refreshToken: string;
  }

  interface User extends FshUser {
    accessToken: string;
    refreshToken: string;
  }

  interface JWT {
    user: FshUser;
    accessToken: string;
    refreshToken: string;
    accessTokenExpires: number;
  }
}
