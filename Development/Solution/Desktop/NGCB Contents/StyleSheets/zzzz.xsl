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
              <xsl:value-of select="tTableName/ImagePath" />
            </xsl:attribute>
          </IMG>
          <xsl:text> </xsl:text>
          <font CLASS="zname">
            <xsl:value-of select="tTableName/Name"/>
          </font>
          <hr></hr>
          
          <font face="Verdana" size="2" >
            <b>
              <xsl:value-of select="tTableName/Code"/>
            </b>
          </font>
          
<!--START of table Information-->
            <BR/>
            <BR/>
            <BR/>
            <DIV CLASS="zgroup">
              Details
            </DIV>
            <BR/>
            <TABLE BORDER="0" CELLPADDING="0" CELLSPACING="4">
              
              <TR height="18">
                <TD CLASS="zproperty">Company:</TD>
                <TD CLASS="zvalue" align="right">
                  <xsl:value-of select="tTableName/Company"/>
                </TD>
              </TR>
              
            </TABLE>
<!--END of table Information-->
          </P>
          Last Updated By:
          <br/>
          Date: <xsl:value-of select="ms:format-date(tTableName/DateModified, 'MMM dd, yyyy')"/>
          <BR/>
          <BR/>
          <BR/>
        </xsl:for-each>
      </BODY>
    </HTML>
  </xsl:template>
</xsl:stylesheet>