export interface Role {
  id: string;
  name: string;
  description?: string;
}

export interface Permission {
  permission: string;
  description?: string;
}

export interface RoleWithPermissions extends Role {
  permissions: Permission[];
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
}

export interface UpdateRolePermissionsRequest {
  roleId: string;
  permissions: string[];
}

export interface UserRole {
  roleId: string;
  roleName: string;
  enabled: boolean;
}
