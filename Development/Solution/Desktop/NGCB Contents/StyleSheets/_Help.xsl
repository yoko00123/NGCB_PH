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
              <xsl:value-of select="tMenu/ImagePath" />
            </xsl:attribute>
          </IMG>
          <xsl:text> </xsl:text>
          <font class="zname">
             <xsl:value-of select="tMenu/Name"/>
          </font>
          <hr></hr>
          <br/>
          <br/>

          <xsl:if test="tMenu/Description">
            <div CLASS="zhelpgroup">
              Description
            </div>

            <pre>
              <xsl:value-of select="tMenu/Description"/>
              </pre>
            
                      </xsl:if>


          <xsl:if test="tMenuTabField">
            <div CLASS="zhelpgroup">
              Menu Field
            </div>
            <table border="1" cellpadding="4" cellspacing="0" width="100%">
              <tr>
                <th>
                  Menu Tab
                </th>
                <th width="16px">
                </th>
                <th>
                  Field Name
                </th>
                <th>
                  Description
                </th>
              </tr>
              <xsl:for-each select="tMenuTabField">
                <tr>
                  <td width="90px">
                    <xsl:value-of select="MenuTab"/>
                  </td>
                  <td>
                    <xsl:if test="ImagePath">
                    <IMG width="16" height="16">
                      <xsl:attribute name="src">
                        <xsl:value-of select="ImagePath"/>
                      </xsl:attribute>
                    </IMG>
                    </xsl:if >
                  </td>
                  <td width="150px">
                    <xsl:value-of select="Label"/>
                  </td>
                  <td>
                    <xsl:value-of select="Description"/>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
		  <BR/>
		  <hr></hr>
        </xsl:for-each>
      </BODY>
    </HTML>
  </xsl:template>
</xsl:stylesheet>