module SaberBase
  def ingest_test_data_to_souceone
    begin
      if system('C:\SaberAgent\Saber\AutomationFramework\ES1Automation\Main\Saber\IngestTestDataToSourceOneArchiveAndIndex\bin\debug\IngestTestDataToSourceOneArchiveAndIndex.exe')
        puts 'Successfully ingest the test data.'    
      else
        raise 'Ingest test data to SourceOne failed.'
      end
    rescue
      puts 'Error!' + $!.to_s
    end        
  end
end