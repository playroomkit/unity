### PlayroomKit SDK for Unity

This SDK is a wrapper over PlayroomKit JS. Currently, it only supports WebGL exported games.
See [PlayroomKit Unity docs](https://docs.joinplayroom.com/usage/unity) for more information.

### Creating a new release

Tag and push the tag to create a new release. The tag should be in the format `v0.Y.Z`. For example, `v0.0.20`.

```bash
git tag v0.Y.Z
git push origin --tags
```

This will create a draft release [on GitHub](https://github.com/asadm/playroom-unity/releases). Edit the release to add release notes and publish it.