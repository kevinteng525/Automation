<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method='html' version='1.0' encoding='UTF-8' indent='yes'/>

  <xsl:template match="test-results">
    <html>
      <body>
        <center>
          <h2>SPO Automation Test Report</h2>
        </center>
        <br></br>
        <h3>Result Summary</h3>
        <table border="1" width="100%">
          <tr bgcolor="#9acd32">
            <th align="left">Total</th>
            <th align="left">Failed</th>
			<th align="left">Not Run</th>
			<th align="left">Skipped</th>
			<th align="left">Time</th>
          </tr>
          <tr>
            <td>
              <xsl:value-of select="@total"/>
            </td>
			<xsl:if test="@failures > '0'">
                <td bgcolor ="red">
                  <xsl:value-of select="@failures"/>
                </td>
			</xsl:if>
            <xsl:if test="@failures = '0'">
                <td>
                  <xsl:value-of select="@failures"/>
                </td>
			</xsl:if>
            <td>
              <xsl:value-of select="@not-run"/>
            </td>
			<td>
              <xsl:value-of select="@skipped"/>
            </td>
			<td>
              <xsl:value-of select="@time"/>
            </td>
          </tr>
        </table>
        <br></br>
        <h3>Details</h3>

		<xsl:for-each select="test-suite/results/test-suite/results/test-suite">
         <table border="1" width="100%">
          <tr bgcolor="#9acd32">
			<th align="left">Category</th>
            <th align="left">Test Name</th>
			<th align="left">Description</th>
			<th align="left">Duration</th>
            <th align="left">Result</th>
            <th align="left">Error message</th>
			<th align="left">Stack Trace</th>
          </tr>
          <xsl:for-each select="results/test-case">
            <tr>
              <td>
                <xsl:value-of select="categories/category/@name"/>
              </td>
              <td>
                <xsl:value-of select="@name"/>
              </td>
			  <td>
                <xsl:value-of select="@description"/>
              </td>
			  <td>
                <xsl:value-of select="@time"/>
              </td>
              <xsl:if test="@result = 'Failure'">
                <td bgcolor ="red">
                  <xsl:value-of select="@result"/>
                </td>
                <td>
                  <xsl:value-of select="failure/message"/>
                </td>
				<td>
                  <xsl:value-of select="failure/stack-trace"/>
                </td>
              </xsl:if>
              <xsl:if test="@result = 'Success'">
                <td bgcolor ="lime">
                  <xsl:value-of select="@result"/>
                </td>
                <td>
                  N/A
                </td>
				<td>
                  N/A
                </td>
              </xsl:if>
            </tr>
          </xsl:for-each>
         </table>
		 <br></br>
		</xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>