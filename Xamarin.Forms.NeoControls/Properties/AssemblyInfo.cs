using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

//Linker safe
[assembly: Preserve(AllMembers = true)]

//Custom xaml schema <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/custom-namespace-schemas#defining-a-custom-namespace-schema"/>
[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Xamarin.Forms.NeoControls")]

//Recommended prefix <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/custom-prefix"/>
[assembly: XmlnsPrefix("http://neocontrols.com/schemas/xaml", "neo")]

//Xaml compilation <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/xamlc"/>
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]