export interface SidebarLink {
  title: string;
  slug: string;
}

export interface SidebarGroup {
  label: string;
  items: SidebarLink[];
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
    ],
  },
  {
    label: 'Modules',
    items: [
      { title: 'Identity', slug: 'modules/identity' },
      { title: 'Multitenancy', slug: 'modules/multitenancy' },
      { title: 'Auditing', slug: 'modules/auditing' },
    ],
  },
  {
    label: 'Building Blocks',
    items: [
      { title: 'Overview', slug: 'building-blocks' },
    ],
  },
  {
    label: 'Guides',
    items: [
      { title: 'Adding a Feature', slug: 'adding-a-feature' },
      { title: 'Adding a Module', slug: 'adding-a-module' },
    ],
  },
  {
    label: 'Deployment',
    items: [
      { title: 'Overview', slug: 'deployment' },
    ],
  },
  {
    label: 'Contributing',
    items: [
      { title: 'Guide', slug: 'contributing' },
    ],
  },
];
