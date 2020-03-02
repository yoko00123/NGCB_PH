<?xml version='1.0'?>
<xsl:stylesheet version="1.0" 
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   
		xmlns:ms="urn:schemas-microsoft-com:xslt">
  <xsl:param name="stylesheet"/>
  <xsl:param name="image"/>
  <xsl:template match="/">


    <html>
      <head>
        <link rel="stylesheet" type="text/css">
          <xsl:attribute name="href">
            <xsl:value-of select="$stylesheet" />
          </xsl:attribute>
        </link>
      </head>
      <body bgcolor="white">
        <center>
          <img>
            <xsl:attribute name="src">
              <xsl:value-of select="$image" />
            </xsl:attribute>
          </img>
        </center>
        
          <!-- 
					<font class="zname">
						<xsl:value-of select="tBranch/Name"/>
					</font>
					<hr></hr>
					<font face="Verdana" size="2" >
						<b>
							<xsl:value-of select="tBranch/Company"/>
						</b>
					</font>
          -->
			
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>