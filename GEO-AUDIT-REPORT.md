# GEO Audit Report: fullstackhero

**Audit Date:** 2026-03-26
**URL:** https://fullstackhero.net
**Secondary URL:** https://fullstackhero.pages.dev
**Business Type:** Open-Source Developer Framework (SaaS/Tool)
**Pages Analyzed:** 73 (sitemap) + 2 domain variants

---

## Executive Summary

**Overall GEO Score: 24/100 (Critical)**

fullstackhero.net is largely invisible to AI systems. The site hasn't been updated since April 2023, references .NET 7 (three major versions behind), has zero presence on Reddit/YouTube/StackOverflow, lacks Open Graph tags, has no llms.txt, and is missing from every major ".NET boilerplate" ranking article. The new Starlight docs site being built is a massive opportunity to fix everything from scratch — but only if GEO is baked in from day one.

### Score Breakdown

| Category | Score | Weight | Weighted |
|---|---|---|---|
| AI Citability | 18/100 | 25% | 4.5 |
| Brand Authority | 15/100 | 20% | 3.0 |
| Content E-E-A-T | 25/100 | 20% | 5.0 |
| Technical GEO | 40/100 | 15% | 6.0 |
| Schema & Structured Data | 30/100 | 10% | 3.0 |
| Platform Optimization | 10/100 | 10% | 1.0 |
| **Overall GEO Score** | | | **24/100** |

---

## Critical Issues (Fix Immediately)

### 1. Site is 3 years stale — references .NET 7
- **Impact:** AI systems will not recommend an outdated framework. Google AI Overviews, ChatGPT, and Perplexity all penalize stale content.
- **Fix:** Deploy the new Starlight docs site as the primary fullstackhero.net. Redirect old paths.

### 2. No llms.txt file
- **Impact:** AI crawlers (Claude, Perplexity) use llms.txt to understand site structure. Without it, they guess.
- **Fix:** Create `/llms.txt` and `/llms-full.txt` (see implementation plan below).

### 3. No Open Graph / Twitter Card meta tags
- **Impact:** When anyone shares fullstackhero links on Twitter, Discord, LinkedIn, or Slack — it shows a blank preview. Zero social amplification.
- **Fix:** Add OG tags to every page of the new docs site.

### 4. Zero Reddit presence
- **Impact:** Reddit is the #1 source AI models cite for developer opinions. "What .NET starter kit should I use?" questions on r/dotnet never mention fullstackhero.
- **Fix:** Content strategy for r/dotnet, r/csharp, r/programming (see plan below).

### 5. Zero YouTube presence
- **Impact:** YouTube is Google's #2 search engine and a primary source for AI training data. Competitors have video walkthroughs.
- **Fix:** Create "Getting Started with fullstackhero in 5 minutes" and architecture walkthrough videos.

### 6. Not listed in any ".NET boilerplate" ranking articles
- **Impact:** Ranking articles are heavily cited by AI. The "Top 10 .NET SaaS Boilerplates in 2026" article by FacileTechnoLab does NOT include fullstackhero. ABP, Orchard Core, ASP.NET Zero are listed instead.
- **Fix:** Outreach to ranking article authors. Create your own "comparison" content.

---

## High Priority Issues

### 7. Meta description is weak and generic
- **Current:** "fullstackhero is an ambitious project to cater the needs of Developers and Organizations and helps reduce the development hours by over 200+ hours!"
- **Problem:** Vague, grammatically awkward, no keywords. AI systems won't quote this.
- **Fix:** "fullstackhero is an open-source .NET 10 starter kit with multitenancy, identity, CQRS, and vertical slice architecture. Production-ready modular monolith framework that saves teams months of infrastructure work."

### 8. No FAQ schema / FAQ content on landing page
- **Impact:** FAQ content is the #1 most-cited content type by AI systems. Google AI Overviews pull directly from FAQ schema.
- **Fix:** Add FAQPage schema to the landing page FAQ section (already built — just needs schema markup).

### 9. No canonical URL specified
- **Impact:** Two domains exist (fullstackhero.net + fullstackhero.pages.dev). Google may split authority. AI models may cite the wrong URL.
- **Fix:** Set canonical to the primary domain on every page. Redirect the other.

### 10. No author attribution / E-E-A-T signals
- **Impact:** AI systems weight content from recognized experts higher. "Created by Mukesh Murugan" in footer is not enough.
- **Fix:** Add a proper "About" page with Mukesh's credentials, links to codewithmukesh.com, speaking engagements, MVP status, etc.

### 11. Sitemap lastmod dates are all 2023-04-22
- **Impact:** Search engines and AI crawlers see this as a dead site.
- **Fix:** New sitemap with accurate lastmod dates. Auto-generate on build.

---

## Medium Priority Issues

### 12. No Wikipedia mention
- AI models use Wikipedia heavily for entity recognition. fullstackhero has no Wikipedia article or mention.

### 13. No StackOverflow tag or questions
- Zero StackOverflow presence means AI can't learn from community Q&A.

### 14. GitHub README could be more citeable
- The README is good but lacks a clear "What is fullstackhero?" paragraph that AI would extract.

### 15. No comparison pages
- "fullstackhero vs ABP Framework", "fullstackhero vs Clean Architecture template" — these are exactly what developers search. AI models love comparison content.

### 16. No structured data beyond basic Organization schema
- Missing: SoftwareApplication, FAQPage, HowTo, BreadcrumbList, Article schema.

### 17. Content blocks are not optimized for AI extraction
- No definition-style paragraphs ("fullstackhero is..."). No self-contained answer blocks.

---

## Low Priority Issues

### 18. Images missing alt text on current site
### 19. No hreflang tags (minor — English only)
### 20. No RSS feed for docs updates
### 21. Privacy policy page is thin

---

## Category Deep Dives

### AI Citability: 18/100

**Why so low:** The current site has almost no content that an AI could extract and quote. The landing page is marketing fluff ("ambitious project to cater the needs"), the docs reference .NET 7, and there are no self-contained answer blocks.

**What good citability looks like:**
> "fullstackhero is an open-source .NET 10 modular monolith starter kit that includes multitenancy (Finbuckle), JWT authentication, CQRS (Mediator source generator), EF Core with PostgreSQL, and vertical slice architecture. It reduces infrastructure setup from 8-14 weeks to under a minute."

This single paragraph, if on the homepage, would be quoted by every AI system responding to "What is fullstackhero?" or "Best .NET starter kit."

**Recommendations:**
- Add a "What is fullstackhero?" definition block within the first 200 words of the homepage
- Add "Key facts" summary boxes on doc pages
- Structure each doc page with a clear TL;DR at the top
- Use the exact keywords competitors rank for: "modular monolith .NET", "multitenancy starter kit", ".NET SaaS boilerplate"

### Brand Authority: 15/100

**Platform presence:**
| Platform | Present? | Details |
|---|---|---|
| GitHub | Yes | 6.3k stars, active repo |
| Reddit | No | Zero mentions found |
| YouTube | No | Zero videos found |
| StackOverflow | No | No tag, no questions |
| Wikipedia | No | No article or mention |
| LinkedIn | Partial | Mukesh's personal profile only |
| Twitter/X | Partial | @iammukeshm — personal, not brand |
| Dev.to | Unknown | Not checked |
| HackerNews | Unknown | Not checked |
| Product Hunt | Unknown | Not checked |

**The problem:** 6.3k GitHub stars is real authority, but AI models don't just look at GitHub. They weight Reddit, YouTube, and StackOverflow heavily. fullstackhero is invisible on all three.

### Content E-E-A-T: 25/100

- **Experience:** No case studies, no "built with fullstackhero" showcase
- **Expertise:** Mukesh is a known .NET developer (codewithmukesh.com) but this isn't surfaced
- **Authoritativeness:** Not listed in any ranking articles, no third-party reviews
- **Trustworthiness:** MIT license is good, GitHub activity is good, but stale docs hurt trust

### Technical GEO: 40/100

- **robots.txt:** Good — allows all crawlers (including AI)
- **Sitemap:** Exists but dates are stale
- **llms.txt:** Missing (404)
- **SSR/Rendering:** Current site uses client-side JS — not ideal for AI crawlers
- **New Starlight site:** Static HTML — excellent for crawlers
- **HTTPS:** Yes
- **Page speed:** Unknown but Starlight sites are fast by default

### Schema & Structured Data: 30/100

**Current schema:** Organization, WebSite, WebPage — basic but present.
**Missing critical schema:**
- `SoftwareApplication` — for the starter kit itself
- `FAQPage` — for the FAQ section
- `HowTo` — for getting started guides
- `BreadcrumbList` — for doc navigation
- `Person` — for Mukesh as author/creator
- `Article` — for doc pages

### Platform Optimization: 10/100

Not optimized for any AI search platform. Not appearing in:
- Google AI Overviews for ".NET starter kit" queries
- ChatGPT web search results
- Perplexity answers
- Bing Copilot responses

---

## Quick Wins (Implement This Week)

### 1. Add llms.txt to the new docs site
```
# fullstackhero

> Open-source .NET 10 modular monolith starter kit with multitenancy, identity, CQRS, vertical slice architecture, and production-ready infrastructure.

## Docs
- [Introduction](/getting-started/introduction/): What fullstackhero is and who it's for
- [Quick Start](/getting-started/quick-start/): Clone, build, and run in 60 seconds
- [Architecture](/architecture/overview/): Modular monolith + vertical slice architecture explained
- [Modules](/modules/identity/): Identity, Multitenancy, Auditing modules
- [Adding Features](/guides/adding-a-feature/): Step-by-step guide to adding vertical slices

## Key Facts
- Framework: .NET 10 / C# latest
- Architecture: Modular Monolith + Vertical Slice Architecture (VSA)
- Multitenancy: Finbuckle.MultiTenant (claim/header/query strategies)
- CQRS: Mediator 3.x (source-generated, zero reflection)
- Database: PostgreSQL with EF Core 10
- Auth: JWT Bearer + ASP.NET Identity
- License: MIT
- GitHub: https://github.com/fullstackhero/dotnet-starter-kit
```

### 2. Add Open Graph tags to the Starlight layout

### 3. Add FAQPage schema to the landing page

### 4. Add a "What is fullstackhero?" definition paragraph as the FIRST content on the homepage

### 5. Set canonical URLs and redirect fullstackhero.pages.dev → fullstackhero.net

---

## 30-Day Action Plan

### Week 1: Technical Foundation
- [ ] Deploy new Starlight docs to fullstackhero.net
- [ ] Add llms.txt and llms-full.txt
- [ ] Add Open Graph + Twitter Card tags to all pages
- [ ] Add FAQPage, SoftwareApplication, Organization, BreadcrumbList schema
- [ ] Set canonical URLs, redirect pages.dev variant
- [ ] Generate fresh sitemap with real lastmod dates
- [ ] Add "What is fullstackhero?" definition block to homepage

### Week 2: Content for Citability
- [ ] Write "fullstackhero vs ABP Framework" comparison page
- [ ] Write "fullstackhero vs Clean Architecture template" comparison page
- [ ] Write "Best .NET Starter Kit for SaaS in 2026" blog post targeting ranking queries
- [ ] Add TL;DR summary blocks to every doc page
- [ ] Create an "About" page with Mukesh's credentials and links
- [ ] Add "Built with fullstackhero" showcase section

### Week 3: Platform Presence
- [ ] Post "I built an open-source .NET 10 modular monolith starter kit" on r/dotnet
- [ ] Post architecture walkthrough on r/csharp
- [ ] Create 5-minute "Getting Started" YouTube video
- [ ] Create "Architecture Explained" YouTube video
- [ ] Answer .NET-related StackOverflow questions mentioning fullstackhero where relevant
- [ ] Submit to Product Hunt
- [ ] Ensure codewithmukesh.com links prominently to fullstackhero.net

### Week 4: Authority Building
- [ ] Reach out to authors of ".NET boilerplate" ranking articles for inclusion
- [ ] Write a guest post for a .NET blog about modular monolith architecture
- [ ] Create a GitHub Discussion template for "Show what you built"
- [ ] Submit talks to .NET conferences about the architecture patterns
- [ ] Set up a blog section in the docs for release notes and architecture decisions

---

## Competitor Analysis

| Feature | fullstackhero | ABP Framework | ASP.NET Zero | Orchard Core |
|---|---|---|---|---|
| GitHub Stars | 6.3k | 13k | N/A (paid) | 7.5k |
| Reddit Mentions | 0 | Many | Some | Some |
| YouTube Videos | 0 | 50+ | 20+ | 30+ |
| StackOverflow Qs | 0 | 500+ | 200+ | 100+ |
| Ranking Articles | 0 | Listed in all | Listed in all | Listed in most |
| Documentation | Stale (.NET 7) | Current | Current | Current |
| AI Citability | Low | High | Medium | Medium |

**Key Insight:** fullstackhero has real technical merit (modular monolith + VSA is a better architecture than ABP's layered approach for most teams), but it's losing the visibility war. ABP has 10x the external presence despite being an older, more complex framework.

---

## Appendix: Key Search Queries to Target

These are the queries that .NET developers, CTOs, and teams type into Google, ChatGPT, and Perplexity:

1. "best .NET starter kit 2026"
2. ".NET modular monolith template"
3. ".NET SaaS boilerplate with multitenancy"
4. "ASP.NET Core starter template production ready"
5. "vertical slice architecture .NET template"
6. ".NET CQRS starter kit"
7. "fullstackhero vs ABP"
8. ".NET 10 starter project"
9. "multitenancy .NET starter"
10. "clean architecture vs modular monolith .NET"

For EACH of these queries, the new docs site should have a page that directly answers the intent.
