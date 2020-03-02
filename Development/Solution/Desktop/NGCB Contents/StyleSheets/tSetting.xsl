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
							<xsl:value-of select="tSetting/ImagePath" />
						</xsl:attribute>
					</IMG>
					<xsl:text> </xsl:text>
					<font class="zname">
						<xsl:value-of select="tSetting/Name"/>
					</font>
					<hr></hr>
					<font face="Verdana" size="2" >
						<b>
							<xsl:value-of select="tSetting/Value"/>
						</b>
					</font>
					
					<P></P>
<p>

					<xsl:if test="tSetting/ID_SettingType=1">
            
						<IFRAME width="100%" height="330" border="1">
							<xsl:attribute name="src">
                <xsl:value-of select="tSetting/Value" />
              </xsl:attribute>
						</IFRAME>
					</xsl:if>
					</p>
<hr></hr>
		  
<p>
		  
<xsl:if test="tSetting/Comment">
				  
<DIV CLASS="zgroup">
		Comment
</DIV>
			<PRE>
            <xsl:value-of select="tSetting/Comment"/>
			</PRE>
		
</xsl:if>	

</p>	
					
				</xsl:for-each>
			</BODY>
		</HTML>
	</xsl:template>
</xsl:stylesheet>