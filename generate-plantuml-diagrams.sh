#!/bin/sh
echo "LOG: Generate Plantuml Diagrams"
exportFormat="-svg"
#monochrome="true"
extra="-SdefaultFontSize=20"
#extra="-SdefaultFontName=Times New Roman -SdefaultFontSize=10"

sudo apt install graphviz

for aFile in `find docs -name "*.puml" -type f`;
do
  #-Smonochrome=$monochrome
    echo "Processing file: $aFile"

  # Create svg/ folder inside that directory if it doesn't exist
  mkdir -p "svg"
  fileName=$(basename "$aFile" .puml)
  echo "Running command: java -jar libs/plantuml-1.2025.2.jar $extra $exportFormat -o 'svg/$fileName' '$aFile'"
	java -jar libs/plantuml-1.2025.2.jar $extra $exportFormat -o "svg/$fileName" "$aFile"
done

echo "Finished"