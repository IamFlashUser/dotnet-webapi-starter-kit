"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  LayoutDashboard,
  Building2,
  Users,
  Shield,
  Settings,
  Hexagon,
} from "lucide-react";
import {
  Sidebar,
  SidebarHeader,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenuItem,
  SidebarMenuButton,
  useSidebar,
} from "@/components/ui/sidebar";

const navItems = [
  {
    label: "Overview",
    items: [
      { title: "Dashboard", href: "/dashboard", icon: LayoutDashboard },
    ],
  },
  {
    label: "Management",
    items: [
      { title: "Tenants", href: "/tenants", icon: Building2 },
      { title: "Users", href: "/users", icon: Users },
      { title: "Roles", href: "/roles", icon: Shield },
    ],
  },
  {
    label: "System",
    items: [
      { title: "Settings", href: "/settings", icon: Settings },
    ],
  },
];

export function AppSidebar() {
  const pathname = usePathname();
  const { open } = useSidebar();

  return (
    <Sidebar>
      <SidebarHeader>
        <Link href="/dashboard" className="flex items-center gap-2">
          <Hexagon className="h-6 w-6 text-sidebar-primary" />
          {open && (
            <span className="text-lg font-bold tracking-tight">
              FullStackHero
            </span>
          )}
        </Link>
      </SidebarHeader>

      <SidebarContent>
        {navItems.map((group) => (
          <SidebarGroup key={group.label}>
            <SidebarGroupLabel>{group.label}</SidebarGroupLabel>
            {group.items.map((item) => (
              <SidebarMenuItem key={item.href}>
                <Link href={item.href}>
                  <SidebarMenuButton isActive={pathname.startsWith(item.href)}>
                    <item.icon className="h-4 w-4 shrink-0" />
                    {open && <span>{item.title}</span>}
                  </SidebarMenuButton>
                </Link>
              </SidebarMenuItem>
            ))}
          </SidebarGroup>
        ))}
      </SidebarContent>

      <SidebarFooter>
        {open && (
          <p className="text-xs text-sidebar-foreground/50">
            fullstackhero.net
          </p>
        )}
      </SidebarFooter>
    </Sidebar>
  );
}
