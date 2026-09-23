<div align="center">
    <img src="https://docs.joinplayroom.com/_next/image?url=%2F_next%2Fstatic%2Fmedia%2Funity-blog.16612f8c.png&w=3840&q=75" width="100%" style="border-radius: 12px">
    <h1 style="margin-top:8px">PlayroomKit SDK for Unity</h1>
   <p>The easiest multiplayer infrastructure for the web</p>
   <a href="https://github.com/playroomkit/unity/releases/latest"><img src="https://img.shields.io/github/v/release/playroomkit/unity?label=Download%20latest%20%28stable%29&amp;style=for-the-badge&amp;color=green" /></a>
   <br />
   <a href="https://github.com/playroomkit/unity/releases"><img src="https://img.shields.io/github/v/release/playroomkit/unity?include_prereleases&amp;label=Download%20latest%20%28beta%29&amp;style=for-the-badge" /></a>
   <br />
   <a href="https://docs.joinplayroom.com/usage/unity"><img src="https://img.shields.io/static/v1?label=Docs&amp;message=API%20Ref&amp;color=000&amp;style=for-the-badge" /></a>
   <a href="https://discord.gg/HGkSRAD8"><img src="https://img.shields.io/static/v1?label=Discord&message=Join&color=7289da&style=for-the-badge" /></a>
</div>

<br/>
<br/>

This SDK is a wrapper over PlayroomKit JS. Currently, it only supports WebGL exported games. The API is meant to closely mirror the PlayroomKit JavaScript SDK.

<!-- Start SDK Installation -->
## Installation and Usage

See [PlayroomKit Unity docs](https://docs.joinplayroom.com/usage/unity) on how to use this SDK in your Unity project.

The SDK is distributed as a UPM package (not a `.unitypackage`). Source lives under `Packages/com.playroomkit.sdk`, and samples are under `Packages/com.playroomkit.sdk/Samples~`.

This beta version of the SDK might undergo changes that could break compatibility with previous versions, even without a major version update. To ensure stability, it's advisable to fix the usage to a particular package version. By doing so, you'll consistently install the same version and avoid unexpected changes, unless you deliberately seek the latest updates.

<!-- End SDK Installation -->

<!-- Start SDK Contribution -->
## Contribution

### Reporting issues

You can search for help, or ask the community, in our [Discord channel](https://discord.gg/HGkSRAD8).

Found a bug, or want us to implement something? [Create an Issue](https://github.com/playroomkit/unity/issues/new) on GitHub.

### Creating a new release

Releases are handled by release-please. Merges to `main` create or update a release PR; merging that PR cuts the tag, updates `CHANGELOG.md`, and publishes the GitHub release (the workflow also attaches the packed UPM artifact).

### Contribute code to this project

Read [CONTRIBUTING.md](./CONTRIBUTING.md) for more information on how to contribute code to this project.

### Examples

Example(s) of the SDK are in the [Samples](https://github.com/playroomkit/unity/tree/main/Packages/com.playroomkit.sdk/Samples~) folder

<!-- End SDK Contribution -->

### Limitations
Currently, there's no support for using the native platforms. We'd love to hear ideas and plans to implement PlayroomKit for Unity on native platforms. Please join [this Discord](https://discord.gg/uDHxeRYhRe) to discuss!
