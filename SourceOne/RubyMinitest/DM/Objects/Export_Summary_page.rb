require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

class ExportSummaryPage < BrowserContainer
  
  #Verify whether the export name is created.
  def validExportName(ename)
    export_grid_table.text.include? ename
  end
  
  private
  
  #Export grid table.
  def export_grid_table
    @browser.div(:class => 'gridx gridxDesktop')
  end
  
end
  