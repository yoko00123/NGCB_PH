<?xml version='1.0'?>
<xsl:stylesheet version="1.0" 
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   
		xmlns:ms="urn:schemas-microsoft-com:xslt">
  <xsl:param name="stylesheet"/>
  <xsl:template match="/">
    <HTML>
      <HEAD>
        <link rel="stylesheet" type="text/css">
          <xsl:attribute name="href">
            <xsl:value-of select="$stylesheet" />
          </xsl:attribute>
        </link>
      </HEAD>
			<BODY>
				<xsl:for-each select="NewDataSet">
					<IMG width="64" height="64">
						<xsl:attribute name="src">
							<xsl:value-of select="tIndustry/ImagePath" />
						</xsl:attribute>
					</IMG>
					<xsl:text> </xsl:text>
					<font class="zname">
						<xsl:value-of select="tIndustry/Name"/>
					</font>
					<hr></hr>
					<font face="Verdana" size="2" >
						<b>
							<xsl:value-of select="tIndustry/Code"/>
						</b>
					</font>
				</xsl:for-each>
			</BODY>
		</HTML>
	</xsl:template>
</xsl:stylesheet>