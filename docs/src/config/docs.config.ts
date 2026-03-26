export interface SidebarLink {
  title: string;
  slug: string;
}

export interface SidebarSubGroup {
  label: string;
  items: SidebarLink[];
}

export type SidebarItem = SidebarLink | SidebarSubGroup;

export interface SidebarGroup {
  label: string;
  items: SidebarItem[];
}

export function isSubGroup(item: SidebarItem): item is SidebarSubGroup {
  return 'items' in item && !('slug' in item);
}

export const sidebar: SidebarGroup[] = [
  {
    label: 'Getting Started',
    items: [
      { title: 'Introduction', slug: 'introduction' },
      { title: 'Prerequisites', slug: 'prerequisites' },
      { title: 'Quick Start', slug: 'quick-start' },
      { title: 'Project Structure', slug: 'project-structure' },
    ],
  },
  {
    label: 'Architecture',
    items: [
      { title: 'Overview', slug: 'architecture' },
      { title: 'CQRS Pattern', slug: 'cqrs' },
      { title: 'Domain Events', slug: 'domain-events' },
      { title: 'Specifications', slug: 'patterns/specifications' },
      { title: 'Outbox & Inbox', slug: 'patterns/outbox-inbox' },
      { title: 'Module System', slug: 'patterns/module-system' },
    ],
  },
  {
    label: 'Modules',
    items: [
      {
        label: 'Identity',
        items: [
          { title: 'Overview', slug: 'modules/identity/overview' },
          { title: 'Users', slug: 'modules/identity/users' },
          { title: 'Roles & Permissions', slug: 'modules/identity/roles-permissions' },
          { title: 'Authentication', slug: 'modules/identity/authentication' },
          { title: 'Sessions & Groups', slug: 'modules/identity/sessions-groups' },
        ],
      },
      {
        label: 'Multitenancy',
        items: [
          { title: 'Overview', slug: 'modules/multitenancy/overview' },
          { title: 'Provisioning', slug: 'modules/multitenancy/provisioning' },
        ],
      },
      {
        label: 'Auditing',
        items: [
          { title: 'Overview', slug: 'modules/auditing/overview' },
          { title: 'Querying Audits', slug: 'modules/auditing/querying' },
        ],
      },
    ],
  },
  {
    label: 'Building Blocks',
    items: [
      { title: 'Overview', slug: 'building-blocks/overview' },
      { title: 'Core', slug: 'building-blocks/core' },
      { title: 'Persistence', slug: 'building-blocks/persistence' },
      { title: 'Web', slug: 'building-blocks/web' },
      { title: 'Caching', slug: 'building-blocks/caching' },
      { title: 'Eventing', slug: 'building-blocks/eventing' },
      { title: 'Jobs', slug: 'building-blocks/jobs' },
      { title: 'Mailing', slug: 'building-blocks/mailing' },
      { title: 'Storage', slug: 'building-blocks/storage' },
      { title: 'Shared', slug: 'building-blocks/shared' },
    ],
  },
  {
    label: 'Cross-Cutting Concerns',
    items: [
      { title: 'Auth & Authorization', slug: 'cross-cutting/authentication-authorization' },
      { title: 'Multitenancy Deep Dive', slug: 'cross-cutting/multitenancy-deep-dive' },
      { title: 'Exception Handling', slug: 'cross-cutting/exception-handling' },
      { title: 'Observability', slug: 'cross-cutting/observability' },
      { title: 'Rate Limiting', slug: 'cross-cutting/rate-limiting' },
      { title: 'Security Headers', slug: 'cross-cutting/security-headers' },
    ],
  },
  {
    label: 'Guides',
    items: [
      { title: 'Adding a Feature', slug: 'guides/adding-a-feature' },
      { title: 'Adding a Module', slug: 'guides/adding-a-module' },
      { title: 'Configuration Reference', slug: 'guides/configuration' },
      { title: 'Testing', slug: 'guides/testing' },
    ],
  },
  {
    label: 'Deployment',
    items: [
      { title: 'Overview', slug: 'deployment/overview' },
      { title: 'Docker', slug: 'deployment/docker' },
      { title: '.NET Aspire', slug: 'deployment/aspire' },
      { title: 'AWS with Terraform', slug: 'deployment/aws-terraform' },
      { title: 'CI/CD', slug: 'deployment/ci-cd' },
    ],
  },
  {
    label: 'Contributing',
    items: [
      { title: 'Guide', slug: 'contributing' },
    ],
  },
];
