<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
  <html lang="en">
    <head>
      <title>Strongwind Test Status Dashboard</title>
      <link rel="stylesheet" type="text/css" href="dashboard.css" media="all"/>
    </head>
    <body>
      <p class="header">
        <span id="documentName">Strongwind Test Status Dashboard</span><br/>
      </p>
      <p class="summaryTop">
        <span id="percentPassed">Passed: </span>
        <xsl:value-of select="dashboard/percentPassed"/>
        <br/>
        <span id="totalTime">Total Time: </span>
        <xsl:value-of select="dashboard/totalTime"/>
      </p>
      <table>
      <tr>
        <th id="controlNumber"></th>
        <th id="controlName">Control</th>
        <th id="status">Status</th>
        <th id="time">Elapsed Time</th>
      </tr>
      <xsl:for-each select="dashboard/control">
        <tr>
          <td><xsl:number/></td>
          <xsl:variable name="controlName" select="name"/>
          <td><a href="reports/{$controlName}"><xsl:value-of select="$controlName"/></a></td>
          <xsl:variable name="status" select="status"/>
          <xsl:choose>
            <xsl:when test="$status = -1">
              <td><center><img src="grey.png" alt="not run" title="not run" height="22" width="22"/></center></td>
            </xsl:when>
            <xsl:when test="$status = 0">
              <td><center><img src="green.png" alt="pass" title="pass" height="22" width="22"/></center></td>
            </xsl:when>
            <xsl:otherwise>
              <td><center><img src="red.png" alt="fail" title="fail" height="22" width="22"/></center></td>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:variable name="time" select="time"/>
          <xsl:choose>
            <xsl:when test="$status = 0">
              <td><xsl:value-of select="time"/></td>
            </xsl:when>
            <xsl:otherwise>
              <td></td>
            </xsl:otherwise>
          </xsl:choose>
        </tr>
      </xsl:for-each>
      </table>
      <p class="summaryTop">
        <span id="percentPassed">Passed: </span>
        <xsl:value-of select="dashboard/percentPassed"/>
        <br/>
        <span id="totalTime">Total Time: </span>
        <xsl:value-of select="dashboard/totalTime"/>
      </p>
    </body>
  </html>
</xsl:template>
</xsl:stylesheet>
