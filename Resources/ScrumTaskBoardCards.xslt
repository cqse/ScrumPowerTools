<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />

  <xsl:template match="/WorkItems">
    <html>

      <head>
        <style>
          body { font-family: arial; }

          .card, .userStoryCard, .bugCard { border: solid 1px black; width: 8cm; height: 5cm; display: inline; margin: 2mm; position: relative; }

          .title { background-color: blue; color: white; font-weight: bold; text-align: center; }

          .bugCard .title { background-color: red; }

          .description, .title, .estimation, .sprintDetails { padding: 2mm; }

          .estimation { position: absolute; bottom: 0px; right: 0px; font-size: 24px; font-weight: bold; }

          .sprintDetails { position: absolute; bottom: 0px; left: 0px; font-size: 10px; color: lightgray; }
        </style>
      </head>

      <body>

        <xsl:for-each select="WorkItem">

          <div class="card">

            <xsl:choose>
              <xsl:when test="@Type = 'User Story'">
                <xsl:attribute name="class">userStoryCard</xsl:attribute>
              </xsl:when>
              <xsl:when test="@Type = 'Bug'">
                <xsl:attribute name="class">bugCard</xsl:attribute>
              </xsl:when>
              <xsl:otherwise>
                <xsl:attribute name="class">card</xsl:attribute>
              </xsl:otherwise>
            </xsl:choose>
            
            <div class="title"><xsl:value-of select="@Type" />&#160;<xsl:value-of select="@Id" /></div>
            <div class="description"><xsl:value-of select="Fields/Field[@RefName='System.Title']/@Value" /></div>
            <div class="estimation"><xsl:value-of select="Fields/Field[@RefName='Microsoft.VSTS.Scheduling.StoryPoints']/@Value" /> SP</div>
            <div class="sprintDetails">
              <div>Rank&#160;<xsl:value-of select="Fields/Field[@RefName='Microsoft.VSTS.Common.StackRank']/@Value" /></div>
              <div><xsl:value-of select="Fields/Field[@RefName='System.AreaPath']/@Value" /></div>
              <div><xsl:value-of select="Fields/Field[@RefName='System.IterationPath']/@Value" /></div>
            </div>
          </div>
          
        </xsl:for-each>

      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>