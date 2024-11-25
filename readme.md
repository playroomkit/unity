<div align="center">
    <img src="https://docs.joinplayroom.com/_next/image?url=%2F_next%2Fstatic%2Fmedia%2Funity-blog.16612f8c.png&w=3840&q=75" width="100%" style="border-radius: 12px">
    <h1 style="margin-top:8px">PlayroomKit SDK for Unity</h1>
   <p>The easiest multiplayer infrastructure for the web</p>
   <a href="https://docs.joinplayroom.com/usage/unity"><img src="https://img.shields.io/static/v1?label=Docs&message=API Ref&color=000&style=for-the-badge" /></a>
   <a href="https://github.com/playroomkit/unity/releases/latest"><img src="https://img.shields.io/github/downloads/playroomkit/unity/latest/playroomkit.unitypackage?label=Download%20latest%20stable" /></a>
   <a href="https://discord.gg/HGkSRAD8"><img src="https://img.shields.io/static/v1?label=Discord&message=Join&color=7289da&style=for-the-badge" /></a>
</div>

<br/>
<br/>

This SDK is a wrapper over PlayroomKit JS. Currently, it only supports WebGL exported games. The API is meant to closely mirror the PlayroomKit JavaScript SDK.

<!-- Start SDK Installation -->
## Installation and Usage

See [PlayroomKit Unity docs](https://docs.joinplayroom.com/usage/unity) on how to use this SDK in your Unity project.


This beta version of the SDK might undergo changes that could break compatibility with previous versions, even without a major version update. To ensure stability, it's advisable to fix the usage to a particular package version. By doing so, you'll consistently install the same version and avoid unexpected changes, unless you deliberately seek the latest updates.

<!-- End SDK Installation -->

<!-- Start SDK Installation -->
## Contribution

### Reporting issues

You can search for help, or ask the community, in our [Discord channel](https://discord.gg/HGkSRAD8).

Found a bug, or want us to implement something? [Create an Issue](https://github.com/asadm/playroom-unity/issues/new) on GitHub.

### Creating a new release

Tag and push the tag to create a new release. The tag should be in the format `v0.Y.Z`. For example, `v0.0.20`.

```bash
git tag v0.Y.Z
git push origin --tags
```

This will create a draft release [on GitHub](https://github.com/asadm/playroom-unity/releases). Edit the release to add release notes and publish it.

### Learn more

Read more about the PlayroomKit Unity integration and the design behind it.

- [Official announcement blog](https://docs.joinplayroom.com/blog/unityweb)
- [Deep dive into PlayroomKit Unity](https://www.linkedin.com/pulse/building-unity-plugin-javascript-grayhatpk-gynfc/?trackingId=kbv0oZVNT6aLh2TjQ%2FhuVw%3D%3D)

### Examples

Example(s) of the SDK are in the [Examples](https://github.com/asadm/playroom-unity/tree/main/Assets/PlayroomKit/Examples) folder