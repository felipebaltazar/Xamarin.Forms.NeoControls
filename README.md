# Xamarin.Forms.NeoControls

 Neomorphic controls for Xamarin Forms

 [![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.NeoControls.svg)](https://www.nuget.org/packages/Xamarin.Forms.NeoControls/)

## Examples
![example](https://user-images.githubusercontent.com/19656249/80289178-62cdbc00-8713-11ea-9333-5e13ad8bc7fc.gif)


## Getting started

- Install the Xamarin.Forms.NeoControls package

 ```
 Install-Package Xamarin.Forms.NeoControls -Version 1.0.0-pre
 ```

- Add NeoControls namespace to your Xaml page/view

```xaml
xmlns:neo="clr-namespace:Xamarin.Forms.NeoControls;assembly=Xamarin.Forms.NeoControls"
```

- Use the controls

```xml
        <neo:NeoButton Elevation=".25"
                       CornerRadius="70,20,20,20"
                       BackgroundColor="#e3edf7"/>
```

- You can also insert any view inside the neo controls

```xml
        <neo:NeoButton BackgroundColor="#e3edf7">
            
            <StackLayout Orientation="Vertical">
                <Image Source="MyImage.png "/>
                <Label Text="My Button Label"/>
            </StackLayout>
            
        </neo:NeoButton>
```

## Property reference

| Property           | What it does                                                          | Extra info                                                                 |
| ------------------ | --------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| `CornerRadius`     | A `CornerRadius` object representing each individual corner's radius. | Uses the `CornerRadius` struct allowing you to specify individual corners. |
| `Elevation`        | Set this value to chenge element depth effect.                        |                                                                            |
| `InnerView`        | View that will be shown inside the neo control.                       |                                                                            |
| `ShadowBlur`       | Set this value to change shadow blur effect.                          |                                                                            |
| `ShadowDistance`   | Set this value to change shadow distance relative from control.       |                                                                            |
| `DarkShadowColor`  | The Dark color that will be applied on draw the dark shadow.          | This will be applied with `Elevation` property, as Alpha parameter.        |
| `LightShadowColor` | The White color that will be applied on draw the light shadow.        |                                                                            |