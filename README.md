# UnityContainer

A dependency injection container

## Installation

1. Open the Unity Package Manager
2. Click the + button
3. Select "Add package from git URL..."
4. Paste the following URL
```
https://github.com/DreamTapir/UnityContainer.git?path=Packages/UnityContainer
```

## How To Use
1. Place a SceneContainer Prefab in the scene
<img width="336" alt="image" src="https://github.com/user-attachments/assets/46f48793-ea5c-44f7-84e4-6c2853e9f44b" />

2. Assign the [Validate] attribute to the Field or Method want to validate in the MonoBehaviour class
```C#
  public class ExampleClass : MonoBehaviour
  {
      [Validate]
      private IIntProvider _intProvider;
      private IFloatProvider _floatProvider;
      private IColorProvider _colorProvider;

      [Validate]
      public void Validate(List<IFloatProvider> floatProviders, IEnumerable<IColorProvider> colorProviders)
      {
          _floatProvider = floatProviders.Where(f => f.Value > 0).FirstOrDefault();
          _colorProvider = colorProviders.Where(c => c.Color.g == 1f).FirstOrDefault();
      }
  }
```
