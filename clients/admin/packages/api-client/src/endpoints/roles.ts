import type { AxiosInstance } from "axios";
import type { Role, RoleWithPermissions, CreateRoleRequest, UpdateRolePermissionsRequest } from "../types/role";

export function createRoleEndpoints(client: AxiosInstance) {
  return {
    list: () =>
      client.get<Role[]>("/api/v1/identity/roles"),

    get: (id: string) =>
      client.get<RoleWithPermissions>(`/api/v1/identity/roles/${id}`),

    create: (data: CreateRoleRequest) =>
      client.post<string>("/api/v1/identity/roles", data),

    delete: (id: string) =>
      client.delete(`/api/v1/identity/roles/${id}`),

    getPermissions: (id: string) =>
      client.get<RoleWithPermissions>(`/api/v1/identity/roles/${id}/permissions`),

    updatePermissions: (data: UpdateRolePermissionsRequest) =>
      client.put(`/api/v1/identity/roles/${data.roleId}/permissions`, data),
  };
}
