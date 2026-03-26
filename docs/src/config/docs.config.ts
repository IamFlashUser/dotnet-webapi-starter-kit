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
      { title: 'Specification Pattern', slug: 'specification-pattern' },
      { title: 'Outbox & Inbox Pattern', slug: 'outbox-inbox-pattern' },
      { title: 'Module System', slug: 'module-system' },
    ],
  },
  {
    label: 'Modules',
    items: [
      {
        label: 'Identity',
        items: [
          { title: 'Overview', slug: 'identity-module' },
          { title: 'User Management', slug: 'user-management' },
          { title: 'Roles & Permissions', slug: 'roles-and-permissions' },
          { title: 'Authentication', slug: 'authentication' },
          { title: 'Sessions & Groups', slug: 'sessions-and-groups' },
        ],
      },
      {
        label: 'Multitenancy',
        items: [
          { title: 'Overview', slug: 'multitenancy' },
          { title: 'Tenant Provisioning', slug: 'tenant-provisioning' },
        ],
      },
      {
        label: 'Auditing',
        items: [
          { title: 'Overview', slug: 'auditing' },
          { title: 'Querying Audits', slug: 'querying-audits' },
        ],
      },
      {
        label: 'Webhooks',
        items: [
          { title: 'Overview', slug: 'webhooks' },
        ],
      },
    ],
  },
  {
    label: 'Building Blocks',
    items: [
      { title: 'Overview', slug: 'building-blocks-overview' },
      { title: 'Core', slug: 'core-building-block' },
      { title: 'Persistence', slug: 'persistence-building-block' },
      { title: 'Web', slug: 'web-building-block' },
      { title: 'Caching', slug: 'caching' },
      { title: 'Eventing', slug: 'eventing' },
      { title: 'Background Jobs', slug: 'background-jobs' },
      { title: 'Mailing', slug: 'mailing' },
      { title: 'File Storage', slug: 'file-storage' },
      { title: 'Shared Library', slug: 'shared-library' },
      { title: 'HTTP Resilience', slug: 'http-resilience' },
      { title: 'Feature Flags', slug: 'feature-flags' },
      { title: 'Idempotency', slug: 'idempotency' },
      { title: 'Server-Sent Events', slug: 'server-sent-events' },
    ],
  },
  {
    label: 'Cross-Cutting Concerns',
    items: [
      { title: 'Auth & Authorization', slug: 'authentication-and-authorization' },
      { title: 'Multitenancy Deep Dive', slug: 'multitenancy-deep-dive' },
      { title: 'Exception Handling', slug: 'exception-handling' },
      { title: 'Observability', slug: 'observability' },
      { title: 'Rate Limiting', slug: 'rate-limiting' },
      { title: 'Security Headers', slug: 'security-headers' },
    ],
  },
  {
    label: 'Guides',
    items: [
      { title: 'Adding a Feature', slug: 'adding-a-feature' },
      { title: 'Adding a Module', slug: 'adding-a-module' },
      { title: 'Configuration Reference', slug: 'configuration-reference' },
      { title: 'Testing', slug: 'testing' },
    ],
  },
  {
    label: 'Deployment',
    items: [
      { title: 'Overview', slug: 'deployment-overview' },
      { title: 'Docker', slug: 'docker' },
      { title: '.NET Aspire', slug: 'dotnet-aspire' },
      { title: 'AWS Terraform', slug: 'aws-terraform-deployment' },
      { title: 'CI/CD Pipelines', slug: 'ci-cd-pipelines' },
    ],
  },
  {
    label: 'Contributing',
    items: [
      { title: 'Guide', slug: 'contributing' },
    ],
  },
];
