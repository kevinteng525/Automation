class Ranking < ActiveRecord::Base
  private
  def Ranking.initialize_from_json_params(params)
    ranking = Ranking.new    
    ranking.id = params['RankingId']
    ranking.name = params['Name']
    ranking.description = params['Description']
    ranking
  end
  def Ranking.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']    
    hash[:Description] = params['description']     
    JSON.generate(hash)
  end
end
