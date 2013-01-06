<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />

  <xsl:template match="/WorkItems">
    <html>

      <head>
        <style>
          body { font-family: arial; }

          .card { border: solid 1px black; width: 7cm; height: 5cm; display: inline; margin: 2mm; position: relative; }

          .title { background-color: blue; color: white; font-weight: bold; text-align: center; }

          .description, .title, .estimation, .sprintDetails { padding: 2mm; }

          .estimation { position: absolute; bottom: 0px; right: 0px; }

          .sprintDetails { position: absolute; bottom: 0px; left: 0px; font-size: 8px; color: lightgray; }
        </style>
      </head>

      <body>

        <xsl:for-each select="WorkItem">

          <div class="card">
            <div class="title"><xsl:value-of select="@Type" />&#160;<xsl:value-of select="@Id" /></div>
            <div class="description"><xsl:value-of select="Fields/Field[@RefName='System.Description']/@Value" /></div>
            <div class="estimation">??? SP</div>
            <div class="sprintDetails">
              <div><xsl:value-of select="Fields/Field[@RefName='System.AreaPath']/@Value" /></div>
              <div><xsl:value-of select="Fields/Field[@RefName='System.IterationPath']/@Value" /></div>
            </div>
          </div>
          
        </xsl:for-each>

      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>