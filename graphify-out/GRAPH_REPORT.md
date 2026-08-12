# Graph Report - ace-mises  (2026-08-12)

## Corpus Check
- 10 files · ~8,133 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 126 nodes · 118 edges · 15 communities (10 shown, 5 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]

## God Nodes (most connected - your core abstractions)
1. `What You Must Do When Invoked` - 15 edges
2. `restore` - 14 edges
3. `restore` - 14 edges
4. `/graphify` - 14 edges
5. `net9.0` - 9 edges
6. `net9.0` - 9 edges
7. `/Users/abel/Git/ace-mises/src/domain/ace.domain/ace.domain.csproj` - 4 edges
8. `restoreAuditProperties` - 4 edges
9. `project` - 4 edges
10. `restoreAuditProperties` - 4 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Communities (15 total, 5 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.11
Nodes (18): Part A - Structural extraction for code files, Part B - Semantic extraction (parallel subagents), Part C - Merge AST + semantic into final extraction, Step 1 - Ensure graphify is installed, Step 2.5 - Transcribe video / audio files (only if video files detected), Step 2 - Detect files, Step 3 - Extract entities and relationships, Step 4 - Build graph, cluster, analyze, generate outputs (+10 more)

### Community 1 - "Community 1"
Cohesion: 0.11
Nodes (18): configFilePaths, originalTargetFrameworks, outputPath, packagesPath, projectName, projectPath, projectStyle, projectUniqueName (+10 more)

### Community 2 - "Community 2"
Cohesion: 0.11
Nodes (18): restore, configFilePaths, originalTargetFrameworks, outputPath, packagesPath, projectName, projectPath, projectStyle (+10 more)

### Community 3 - "Community 3"
Cohesion: 0.14
Nodes (13): For --cluster-only, For git commit hook, For /graphify add, For /graphify explain, For /graphify path, For /graphify query, For native CLAUDE.md integration, For --update (incremental re-extraction) (+5 more)

### Community 4 - "Community 4"
Cohesion: 0.17
Nodes (11): libraries, packageFolders, /Users/abel/.nuget/packages/, project, frameworks, version, projectFileDependencyGroups, net9.0 (+3 more)

### Community 5 - "Community 5"
Cohesion: 0.18
Nodes (11): Microsoft.NETCore.App, net9.0, privateAssets, assetTargetFallback, frameworkReferences, imports, projectReferences, runtimeIdentifierGraphPath (+3 more)

### Community 6 - "Community 6"
Cohesion: 0.18
Nodes (11): Microsoft.NETCore.App, net9.0, privateAssets, assetTargetFallback, frameworkReferences, imports, projectReferences, runtimeIdentifierGraphPath (+3 more)

### Community 7 - "Community 7"
Cohesion: 0.25
Nodes (7): format, projects, /Users/abel/Git/ace-mises/src/domain/ace.domain/ace.domain.csproj, restore, /Users/abel/Git/ace-mises/src/domain/ace.domain/ace.domain.csproj, frameworks, version

## Knowledge Gaps
- **87 isolated node(s):** `plugin`, `net9.0`, `Microsoft.NET.Sdk`, `format`, `/Users/abel/Git/ace-mises/src/domain/ace.domain/ace.domain.csproj` (+82 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `restore` connect `Community 2` to `Community 4`, `Community 6`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **Why does `restore` connect `Community 1` to `Community 5`, `Community 7`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Why does `What You Must Do When Invoked` connect `Community 0` to `Community 3`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **What connects `plugin`, `net9.0`, `Microsoft.NET.Sdk` to the rest of the system?**
  _87 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.1111111111111111 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.1111111111111111 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.1111111111111111 - nodes in this community are weakly interconnected._