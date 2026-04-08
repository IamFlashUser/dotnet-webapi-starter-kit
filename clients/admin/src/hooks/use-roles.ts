import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { CreateRoleRequest, UpdateRolePermissionsRequest } from "@fsh/api-client";
import { api } from "@/lib/api";

export function useRolesQuery() {
  return useQuery({
    queryKey: ["roles"],
    queryFn: () => api.roles.list().then((r) => r.data),
  });
}

export function useRoleQuery(id: string) {
  return useQuery({
    queryKey: ["roles", id],
    queryFn: () => api.roles.get(id).then((r) => r.data),
    enabled: !!id,
  });
}

export function useRolePermissions(id: string) {
  return useQuery({
    queryKey: ["roles", id, "permissions"],
    queryFn: () => api.roles.getPermissions(id).then((r) => r.data),
    enabled: !!id,
  });
}

export function useCreateRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateRoleRequest) => api.roles.create(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
  });
}

export function useDeleteRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.roles.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
  });
}

export function useUpdateRolePermissions() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateRolePermissionsRequest) =>
      api.roles.updatePermissions(data),
    onSuccess: (_, variables) =>
      queryClient.invalidateQueries({
        queryKey: ["roles", variables.roleId],
      }),
  });
}
