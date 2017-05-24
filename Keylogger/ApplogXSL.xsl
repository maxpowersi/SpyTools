<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
	<html> 
		<body>  
		<h2>SnoopB Keylogger Log file</h2>  
		<table border="1"> 
			<tr bgcolor="Silver">  
				<th>Window Title</th>  
				<th>Process Name</th>  
				<th>Log DataHour</th> 
				<th>Log Data</th> 
			</tr> 
			<xsl:for-each select="ApplDetails/Apps_Log"><xsl:sort select="ApplicationName"/> 
				<tr>  
					<td><xsl:value-of select="ProcessName"/></td>  
					<td><xsl:value-of select="ApplicationName"/>	
					</td><td><xsl:value-of select="LogDataHour"/></td> 
					<td><xsl:value-of select="LogData"/></td>  
				</tr> 
			</xsl:for-each>  
		</table> 
		</body> 
	</html>
</xsl:template></xsl:stylesheet>