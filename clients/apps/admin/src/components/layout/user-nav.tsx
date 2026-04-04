"use client";

import { LogOut, User } from "lucide-react";

export function UserNav() {
  return (
    <div className="flex items-center gap-2">
      <button className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-muted text-muted-foreground hover:bg-accent hover:text-accent-foreground">
        <User className="h-4 w-4" />
        <span className="sr-only">User menu</span>
      </button>
      <button className="inline-flex h-8 w-8 items-center justify-center rounded-md text-muted-foreground hover:bg-accent hover:text-accent-foreground">
        <LogOut className="h-4 w-4" />
        <span className="sr-only">Sign out</span>
      </button>
    </div>
  );
}
