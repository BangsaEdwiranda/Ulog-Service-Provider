using ComponentSpace.Saml2.Configuration;
using ComponentSpace.Saml2.Configuration.Serialization;
using ComponentSpace.Saml2.Metadata.Import;
using System;
using System.IO;

namespace ImportMetadata
{
    /// <summary>
    /// Imports SAML metadata into the SAML configuration.
    /// 
    /// Usage: dotnet ImportMetadata.dll
    /// </summary>
    class Program
    {
        static void Main()
        {
            try
            {
                Console.Write("SAML metadata file to import: ");
                var fileName = Console.ReadLine();

                var metadataToConfiguration = new MetadataToConfiguration();
                var samlConfigurations = metadataToConfiguration.ImportFile(fileName);

                SaveConfiguration(samlConfigurations);
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private static void SaveConfiguration(SamlConfigurations samlConfigurations)
        {
            Console.Write("SAML configuration file [saml.json]: ");

            var fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "saml.json";
            }

            File.WriteAllText(fileName, ConfigurationSerializer.Serialize(samlConfigurations));
        }
    }
}
