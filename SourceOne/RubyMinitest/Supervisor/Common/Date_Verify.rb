require 'time'

class DateValidation

  #s_date format is mm/dd/yy.
  def self.date_verify(s_date)
    array_date = s_date.split("/")
    year = array_date[2].to_i
    month = array_date[0].to_i
    day = array_date[1].to_i
    if year < 1753
      puts "error: The year must greater than 1753."
      return false
    else
      return Date.valid_date?(year, month, day)
    end
  end
  
  #s_date format is mm/dd/yy.
  #verify end date should be greater than start date.
  def self.compare_date(startdate, enddate)
    array_startdate = startdate.split("/")
    array_enddate = enddate.split("/")
    
    s_year = array_startdate[2].to_i
    s_month = array_startdate[0].to_i
    s_day = array_startdate[1].to_i    
    
    e_year = array_enddate[2].to_i
    e_month = array_enddate[0].to_i
    e_day = array_enddate[1].to_i     
    
    if s_year > e_year
      return false
    elsif s_year < e_year
      return true
    else
      if s_month > e_month
        return false
      elsif s_month < e_month
        return true
      else
        if s_day > e_day
          return false
        else
          return true
        end
      end
    end
  end 
end

