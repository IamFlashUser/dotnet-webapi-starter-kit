import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { PaginationParams, CreateUserRequest, UpdateUserRequest } from "@fsh/api-client";
import { api } from "@/lib/api";

export function useUsersQuery(params: PaginationParams) {
  return useQuery({
    queryKey: ["users", params],
    queryFn: () => api.users.list(params).then((r) => r.data),
  });
}

export function useUserQuery(id: string) {
  return useQuery({
    queryKey: ["users", id],
    queryFn: () => api.users.get(id).then((r) => r.data),
    enabled: !!id,
  });
}

export function useCreateUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateUserRequest) => api.users.create(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["users"] }),
  });
}

export function useUpdateUser(id: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateUserRequest) => api.users.update(id, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["users"] }),
  });
}

export function useToggleUserStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.users.toggleStatus(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["users"] }),
  });
}

export function useUserRoles(userId: string) {
  return useQuery({
    queryKey: ["users", userId, "roles"],
    queryFn: () => api.users.getRoles(userId).then((r) => r.data),
    enabled: !!userId,
  });
}

export function useAssignUserRoles(userId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (roles: { roleId: string; enabled: boolean }[]) =>
      api.users.assignRoles(userId, roles),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["users", userId, "roles"] }),
  });
}
