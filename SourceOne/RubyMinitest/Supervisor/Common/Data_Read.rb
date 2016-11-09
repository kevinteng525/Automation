require 'win32ole'

#Description:
# =>This class will help you create an excel object, open the excel file, focus on the selected sheet and close the excel.
class ExcelOpenClose
  #Parameters:
  # =>      excel_dir: It is the path which store the data excel file.
  # =>      sheet_num: It is the excel sheet number.
  # =>          excel: Constant.
  def initialize(excel_dir,sheet_num,excel = WIN32OLE::new('excel.Application'))
    @excel = excel
    @sheet_num = sheet_num
    #open excel.
    @workbook = @excel.Workbooks.Open(excel_dir)
    #select the sheet in excel file.
    @worksheet = @workbook.Worksheets(@sheet_num)
  end
  
  #Close excel file.
  def closeExcel  
    @excel.ActiveWorkbook.Close(0)
    @excel.Quit  
  end
end

#Description:
# =>This class will help you operate the excel file.
class ReadExcel < ExcelOpenClose
  #calculate how many data rows in the sheet.
  def getRow
    _line=1
    #calculate how many rows in the sheet.
    while  @worksheet.Range("a#{_line}").value
      _line = _line + 1
    end
    _row =  _line - 2
    if _row == 0
     puts 'warning: There is no data in this sheet: '+@sheet_num.to_s
    #else
     #puts 'It has '+_row.to_s+' rows data in the data sheet '+@sheet_num.to_s+'.'
    end 
    return _row
  end

  
  def getUserData(line,row_input)
    data = []
    if line == 0
      puts 'There is no data in this sheet.'
      return false   
    elsif row_input > line
      puts 'it only has '+line.to_s+' record(s) in this sheet. you cannot select row '+row_input.to_s+'.'
      return false
    #elsif @sheet_num==1
    else
      array_array = @worksheet.Range("a#{row_input+1}:k#{row_input+1}").value
      data = array_array[0]
      return data
    end  
  end 
  
end

